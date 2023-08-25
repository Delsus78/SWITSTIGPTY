<script setup>
import WelcomeItem from './WelcomeItem.vue';
import StartIcon from "@/components/icons/IconStart.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import {computed, ref} from "vue";
import WaitIcon from "@/components/icons/IconWait.vue";
import store from "@/store";
import * as signalR from "@microsoft/signalr";
import config from "@/config";
import EmbedYoutube from "@/components/EmbedYoutube.vue";
import VotePage from "@/components/VotePage.vue";
import ResultPage from "@/components/ResultPage.vue";
import Spacer from "@/components/Spacer.vue";


const emit = defineEmits(["leave","start","vote","endGame", "nextRound"]);
const gamePhase = ref("not-started");
const game = computed(() => store.state.game);
const player = computed(() => store.state.player);
const selectedYoutubeUrl = ref("");

const setGamePhase = (newVal) => {
    gamePhase.value = newVal;
}

// SIGNALR PART
const connection = new signalR.HubConnectionBuilder()
    .withUrl(config.apiUrl + "hubs/GameHub")
    .configureLogging(signalR.LogLevel.Information) // configurer le niveau de log
    .build();

connection.start().catch(err => console.error(err.toString())).then(() => {
    connection.invoke("JoinGroup", game.value.gameCode, player.value.id).catch(err => console.error(err.toString()));
});

const handleLeave = async () => {
    connection.invoke("LeaveGroup", game.value.gameCode).catch(err => console.error(err.toString())).then(() => {
    }).finally(() => emit('leave', gamePhase.value));
}

connection.on("next-round", (message) => {
    // reset votes
    message.game.players.forEach(player => {
        player.hasVoted = false;
    });

    assignGame(message.game);
    selectedYoutubeUrl.value = game.value.songsUrls[message.indexOfSong];
    // TODO afficher les résultats via le next round ou via le game-ended ? ou autre ?
    setGamePhase("started");
});

connection.on("game-ended", (playersReceived) => {
    assignPlayers(playersReceived);
    setGamePhase("result");
});

connection.on("new-vote", (votingPlayerId) => {
    store.dispatch('setHasVoted', votingPlayerId);
});

connection.on("player-number-changed", (message) => {
    store.dispatch('setPlayerCount', message);
});

// END SIGNALR PART


const assignPlayers = (players) => {
    store.dispatch('setPlayers', players);
}

const assignGame = (game) => {
    store.dispatch('setGame', game);
}

const handleStartGame = () => {
    emit('start');
}

const handleNextRound = () => {
    emit('nextRound');
}

const handleVote = (itemId) => {
    emit('vote', itemId);
}

</script>

<template>
    <div v-if="gamePhase === 'not-started'">
        <WelcomeItem>
            <template #icon>
                <WaitIcon />
            </template>
            <template #heading>Waiting Room</template>

            <p>Share this code with your friends so they can join your game.</p>
            <template #important>{{game.gameCode}}</template>
            <template #playerNumber>
                <div v-if="game.playerCount !== null">
                    {{game.playerCount}} in lobby
                </div>
            </template>
            <div v-if="player.isOwner">
                    <ValidationButton msg="Start" @onClick="handleStartGame"/>
                    <spacer :horizontal='40'/>
                    <ValidationButton msg="leave" @onClick="handleLeave" color="red"/>
            </div>
            <div v-else>
                <ValidationButton msg="leave" @onClick="handleLeave" color="red"/>
            </div>
        </WelcomeItem>
    </div>

    <div v-else-if="gamePhase === 'started'">
        <WelcomeItem>
            <template #heading>GAME TIME</template>
            <template #icon>
                <StartIcon />
            </template>
            <EmbedYoutube :youtube-link="selectedYoutubeUrl"></EmbedYoutube>
            <ValidationButton v-if="player.isOwner" msg="STOOOOP" @onClick="handleNextRound" color="red"/>
        </WelcomeItem>

        <WelcomeItem>
            <template #heading>Vote</template>
            <template #icon>
                <WaitIcon />
            </template>
            <VotePage @vote="handleVote" :items="game.players"></VotePage>
        </WelcomeItem>
    </div>

    <div v-else-if="gamePhase === 'result'">
        <WelcomeItem>
            <template #heading>Results</template>
            <template #icon>
                <StartIcon />
            </template>
            <ResultPage :players="players"></ResultPage>
            <ValidationButton msg="leave" @onClick="handleLeave" color="red"/>
        </WelcomeItem>

    </div>
</template>

<style scoped>

</style>