<script setup>
import {ref, onMounted} from 'vue';
import PlayerResult from "@/components/PlayerResult.vue";
import StartIcon from "@/components/icons/IconStart.vue";
import WelcomeItem from "@/components/WelcomeItem.vue";
import ValidationButton from "@/components/ValidationButton.vue";

const emit = defineEmits(["nextRound", "leave"]);

const handleNextRound = async () => {
    emit('nextRound');
}

// Props
const { isOwner, players, isEndScreen } = defineProps({
    isOwner: {
        type: Boolean,
        default: false
    },
    players: {
        type: Array,
        default: () => []
    },
    isEndScreen: {
        type: Boolean,
        default: false
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
<template>
    <WelcomeItem>
        <template v-if="isEndScreen" #heading>Ending Result</template>
        <template v-else #heading>Round Results</template>
        <template #icon>
            <StartIcon />
        </template>
            <PlayerResult
                    v-for="player in sortedPlayers"
                    :key="player.id"
                    :item="player"
                    v-bind:class="{ 'visible': player.isVisible }"
            />
        <ValidationButton v-if="isOwner" msg="Next Round" @onClick="handleNextRound" color="blue"/>
        <ValidationButton v-if="isEndScreen" msg="Leave" @onClick="emit('leave')" color="red"/>
    </WelcomeItem>
</template>
