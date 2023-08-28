<script setup>
import {computed, ref} from "vue";
import store from "@/store";
import * as signalR from "@microsoft/signalr";
import config from "@/config";
import ResultPage from "@/components/ResultPage.vue";
import WaitPage from "@/components/WaitPage.vue";
import InGamePage from "@/components/InGamePage.vue";
import LoadingPage from "@/components/LoadingPage.vue";


const emit = defineEmits(["leave","vote","endRound", "nextRound"]);
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

    setGamePhase("started");
});

connection.on("end-round", (players) => {
    assignPlayers(players);
    setGamePhase("result");
});

connection.on("game-ended", (playersReceived) => {
    assignPlayers(playersReceived);
    setGamePhase("end-result");
});

connection.on("new-vote", (votingPlayerId) => {
    store.dispatch('setHasVoted', votingPlayerId);
});

connection.on("player-number-changed", (message) => {
    store.dispatch('setPlayerCount', message);
});

// END SIGNALR PART

const handleNextRound = async () => {
    gamePhase.value = "loading";
    emit('nextRound');
}

const assignPlayers = (players) => {
    store.dispatch('setPlayers', players);
}

const assignGame = (game) => {
    store.dispatch('setGame', game);
}

</script>
<template>
    <div v-if="gamePhase === 'loading'">
        <LoadingPage></LoadingPage>
    </div>
    <div v-else-if="gamePhase === 'not-started'">
        <WaitPage :game="game" :isOwner="player.isOwner"
                  @leave="handleLeave" @start="handleNextRound"></WaitPage>
    </div>

    <div v-else-if="gamePhase === 'started'">
        <InGamePage :isOwner="player.isOwner" :youtubeUrl="selectedYoutubeUrl" :players="game.players"
                    @endRound="emit('endRound')" @vote="emit('vote', $event)"></InGamePage>
    </div>

    <div v-else-if="gamePhase === 'result'">
        <ResultPage :isOwner="player.isOwner" :players="game.players"
                    @nextRound="handleNextRound"></ResultPage>
    </div>

    <div v-else-if="gamePhase === 'end-result'">
        <ResultPage :isOwner="false" :players="game.players"
                    isEndScreen @leave="handleLeave"></ResultPage>
    </div>
</template>
<style scoped>

</style>