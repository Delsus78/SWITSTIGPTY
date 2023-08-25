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
const game = computed(() => store.state.game);
const player = computed(() => store.state.player);

const handleCodeRetrieved = async (code, playerName) => {
    // join game
    try {
        const response = await axios.post(config.apiUrl + "Game/" + code + "/join?playerName=" + playerName);

        if (response.status === 200) {
            await store.dispatch('setPlayerIsOwner', false);
            await assignGameStoreAtJoining(response.data);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleGameCreated = async (gameParams, playerName, numberOfManches = 1, pointsPerRightVote = 1, pointsPerVoteFooled = 2) => {
    try {
        const url = `${config.apiUrl}Game?type=${gameParams.type}${(gameParams.type === "genre" ? ("&genre=" + gameParams.genre) : "")}&numberOfManches=${numberOfManches}&pointsPerRightVote=${pointsPerRightVote}&pointsPerVoteFooled=${pointsPerVoteFooled}`;
        const gameInfo = await axios.get(url);

        //join it
        const response = await axios.post(config.apiUrl + "Game/" + gameInfo.data.gameCode + "/join?playerName=" + playerName);

        await store.dispatch('setPlayerIsOwner', true);
        await assignGameStoreAtJoining(response.data);
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleVote = async (playerIdToVote) => {
    try {
        const url = config.apiUrl + "Game/"+ game.value.gameCode + "/"+ player.value.id + "/vote/" + playerIdToVote;
        await axios.post(url);

    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleLeaveGame = async (gamePhase) => {
    try {
        if (gamePhase !== "result")
          await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/leave");

        console.log("Player is leaving the game "+ game.value.gameCode);
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    } finally {
        await store.dispatch('resetAll');
    }
}

const handleEndGame = async () => {
    try {
        if (player.value.isOwner) {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/end");

            console.log("Owner is ending the game "+ game.value.gameCode);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleNextRound = async () => {
    try {
        if (player.value.isOwner) {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/next");

            console.log("Next Round of game : "+ game.value.gameCode);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const assignGameStoreAtJoining = async (joinGameDTO) => {
    await store.dispatch('setGame', joinGameDTO.game);
    await store.dispatch('setPlayer', joinGameDTO.player);
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
    <ConnexionPage v-if="game.gameCode == null"
       @code-retrieved="handleCodeRetrieved"
       @game-created="handleGameCreated"/>
    <GamePage v-else
              @leave="handleLeaveGame"
              @end-game="handleEndGame"
              @next-round="handleNextRound"
              @start="handleNextRound"
              @vote="handleVote"/>
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
