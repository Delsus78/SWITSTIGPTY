<template>
    <div class="player-result">
        <img :src="item?.imageUrl" alt="Joueur" class="player-image">
        <div class="player-info">
            <h2 :class="{'impostor': item?.isImpostor}">{{ item?.name }} {{ item?.isImpostor ? 'IMPOSTOR' : ''}}</h2>
            <div class="voter-bar" :style="{ width: voterPercentage + '%' }"></div>
            <p class="voters">{{ item?.votersNames.join(', ') }}</p>
            <p class="songUrl" v-if="item?.songUrl">Song URL: <a :href="item?.songUrl" target="_blank" rel="noopener noreferrer">{{ item?.songUrl }}</a></p>
            <p class="score">Score: {{ item?.score }}</p>
        </div>
    </div>
</template>
<script setup>

import {computed} from "vue";
import store from "@/store";

const { item } = defineProps({
    item: {
        type: Object,
        required : true
    }
});
const allPlayers = computed(() => store.state.game.players);

const voterPercentage = computed(() => {

    if (item?.votersNames.length === 0) return 0;
    // Retourne le pourcentage de votes reçus par rapport à une valeur max
    return (item?.votersNames.length / allPlayers?.value.length) * 100;
});

</script>
<style scoped>
.player-result {
    display: flex;
    align-items: center;
    padding: 10px;
    border-bottom: 1px solid #ddd;
    opacity: 0;
    transform: translateY(20px);
    transition: opacity 0.5s, transform 0.5s;
}

.player-info {
    flex: 1;
}

.impostor {
    color: var(--vt-c-red-1);
}

.player-image {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    margin-right: 15px;
}

.voter-bar {
    background-color: var(--vt-c-blue-1); /* ou votre couleur de choix */
    height: 10px;
    max-width: 100%;
    transition: width 0.5s;
}

.voters {
    font-size: 12px;
    margin-top: 5px;
}

/* Cette classe sera ajoutée dynamiquement pour animer l'apparition */
.player-result.visible {
    opacity: 1;
    transform: translateY(0);
}

.songUrl {
    font-size: 12px;
    margin-top: 5px;
}
</style>
