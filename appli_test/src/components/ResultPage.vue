<template>
    <div>
        <PlayerResult
                v-for="player in sortedPlayers"
                :key="player.id"
                :item="player"
                v-bind:class="{ 'visible': player.isVisible }"
        />
    </div>
</template>

<script setup>
import {ref, onMounted} from 'vue';
import PlayerResult from "@/components/PlayerResult.vue";

// Props
const { players } = defineProps({
    players: {
        type: Array,
        default: () => []
    }
});

// Data
const sortedPlayers = ref([]);

// Watch for changes in players prop and sort them
onMounted(() => {
    sortedPlayers.value = players.sort((a, b) => a.votersNames.length - b.votersNames.length);

    // Animation logic
    sortedPlayers.value.forEach((player, index) => {
        setTimeout(() => {
            player.isVisible = true;
        }, index * 500);
    });
});

</script>