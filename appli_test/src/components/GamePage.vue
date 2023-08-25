<script setup>
import WelcomeItem from './WelcomeItem.vue';
import StartIcon from "@/components/icons/IconStart.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import {computed, ref} from "vue";
import OwnerPage from "@/components/OwnerPage.vue";
import WaitIcon from "@/components/icons/IconWait.vue";
import store from "@/store";
import * as signalR from "@microsoft/signalr";
import config from "@/config";
import axios from "axios";
import EmbedYoutube from "@/components/EmbedYoutube.vue";
import VotePage from "@/components/VotePage.vue";
import ResultPage from "@/components/ResultPage.vue";
import PlayerResult from "@/components/PlayerResult.vue";


const emit = defineEmits(["leave","start","vote","endGame"]);
const gamePhase = ref("not-started");
const gameCode = computed(() => store.state.gameCode);
const players = computed(() => store.state.players);
const playerId = computed(() => store.state.playerId);
const playerNumber = computed(() => store.state.playerNumber);
const youtubeUrls = computed(() => store.state.youtubeUrls);
const selectedYoutubeUrl = ref("");

const setGamePhase = (newVal) => {
    gamePhase.value = newVal;
}

const { isOwner } = defineProps({
    isOwner: {
        type: Boolean,
        required: true
    }
});

// SIGNALR PART
const connection = new signalR.HubConnectionBuilder()
    .withUrl(config.apiUrl + "hubs/GameHub")
    .configureLogging(signalR.LogLevel.Information) // configurer le niveau de log
    .build();

connection.start().catch(err => console.error(err.toString())).then(() => {
    connection.invoke("JoinGroup", gameCode.value, playerId.value).catch(err => console.error(err.toString()));
});

const handleLeave = async () => {
    connection.invoke("LeaveGroup", gameCode.value).catch(err => console.error(err.toString())).then(() => {
    }).finally(() => emit('leave', gamePhase.value));
}

connection.on("start-game", (message) => {
    selectedYoutubeUrl.value = youtubeUrls.value[message.indexOfSong];
    assignPlayers(message.players);
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
    store.dispatch('assignPlayerNumber', message);
});

// END SIGNALR PART


const assignPlayers = (players) => {
    players.forEach(player => {
        player.hasVoted = false;
    });
    store.dispatch('assignPlayers', players);
}

const handleStartGame = () => {
    emit('start');
}

const handleEndGame = () => {
    emit('endGame');
}

const handleVote = (itemId) => {
    emit('vote', itemId);
}

</script>

<template>
    <div v-if="gamePhase === 'not-started'">
        <OwnerPage v-if="isOwner"
                   @leave="handleLeave" @start="handleStartGame"/>
        <WelcomeItem v-else>
            <template #icon>
                <WaitIcon />
            </template>
            <template #heading>Waiting Room</template>
            <template #playerNumber>
                <div v-if="playerNumber !== null">
                    {{playerNumber}} in lobby
                </div>
            </template>
            Waiting for the owner to start the game with code [ {{gameCode}} ]
            <div></div>
            <ValidationButton msg="leave" @onClick="handleLeave" color="red"/>
        </WelcomeItem>
    </div>
    <div v-else-if="gamePhase === 'started'">
        <WelcomeItem>
            <template #heading>GAME TIME</template>
            <template #icon>
                <StartIcon />
            </template>
            <EmbedYoutube :youtube-link="selectedYoutubeUrl"></EmbedYoutube>
            <ValidationButton v-if="isOwner" msg="STOOOOP" @onClick="handleEndGame" color="red"/>

        </WelcomeItem>
        <WelcomeItem>
            <template #heading>Vote</template>
            <template #icon>
                <WaitIcon />
            </template>
            <VotePage @vote="handleVote" :items="players"></VotePage>
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