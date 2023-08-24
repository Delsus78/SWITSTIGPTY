<script setup>
import HelloWorld from './components/HelloWorld.vue'
import ConnexionPage from './components/ConnexionPage.vue'
import Logo from "@/components/icons/Logo.vue";
import {computed, ref} from "vue";
import { useStore } from "vuex";
import GamePage from "@/components/GamePage.vue";  // Ajouté pour accéder au store
import axios from 'axios';
import config from '@/config.js';

const store = useStore();
const gameCode = computed(() => store.state.gameCode);
const playerId = computed(() => store.state.playerId);
const isOwner = ref(false);

const handleCodeRetrieved = async (code, playerName) => {
    // join game
    try {
        const response = await axios.post(config.apiUrl + "Game/" + code + "/join?playerName=" + playerName);

        if (response.status === 200) {
            isOwner.value = false;
            assignGameStoreAtJoining(response.data);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleGameCreated = async (gameParams, playerName) => {
    try {
        const url = config.apiUrl + "Game?type=" + gameParams.type + (gameParams.type === "genre" ? ("&genre=" + gameParams.genre) : "");
        const gameInfo = await axios.get(url);

        //join it
        const response = await axios.post(config.apiUrl + "Game/" + gameInfo.data.gameCode + "/join?playerName=" + playerName);

        isOwner.value = true;
        assignGameStoreAtJoining(response.data);
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleVote = async (playerIdToVote) => {
    try {
        const url = config.apiUrl + "Game/"+ gameCode.value + "/"+ playerId.value + "/vote/" + playerIdToVote;
        await axios.post(url);

        console.log("Player "+ playerId.value +" is voting for player "+ playerIdToVote);
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleLeaveGame = async () => {
    try {
        if (isOwner.value) {
            await axios.post(config.apiUrl + "Game/" + gameCode.value + "/end");

            console.log("Owner is leaving the game "+ gameCode.value +" and ending it");
        } else {
            await axios.post(config.apiUrl + "Game/" + gameCode.value + "/leave");

            console.log("Player is leaving the game "+ gameCode.value);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    } finally {
        isOwner.value = false;
        resetAll();
    }
}

const handleStartGame = async () => {
    try {
        if (isOwner.value) {
            await axios.post(config.apiUrl + "Game/" + gameCode.value + "/start");

            console.log("Owner is starting the game "+ gameCode.value);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const assignGameStoreAtJoining = (gameSettings) => {
    store.dispatch('assignGameCode', gameSettings.gameCode);
    store.dispatch('assignPlayerNumber', gameSettings.playerCount);
    store.dispatch('assignYoutubeUrls', gameSettings.songsUrls);
    store.dispatch('assignPlayerId', gameSettings.playerId);
}

const resetAll = () => {
    store.dispatch('resetAll');
}
</script>

<template>
  <header>
    <Logo class="logo" />

    <div class="wrapper">
      <HelloWorld msg="SWITSTIGPTY" />
    </div>
  </header>

  <main>
    <ConnexionPage v-if="gameCode == null"
       @code-retrieved="handleCodeRetrieved"
       @game-created="handleGameCreated"/>
    <GamePage v-else @leave="handleLeaveGame" @start="handleStartGame" :is-owner="isOwner" @vote="handleVote"/>
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
  color: var(--color-logo);
  width: 125px;
  height: 125px;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
    color: var(--color-logo);
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
