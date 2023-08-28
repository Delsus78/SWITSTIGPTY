<script setup>

import WaitIcon from "@/components/icons/IconWait.vue";
import Spacer from "@/components/Spacer.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import WelcomeItem from "@/components/WelcomeItem.vue";


const emit = defineEmits(["leave","start"]);

const handleLeave = async () => {
    emit('leave');
}

const handleStartGame = async () => {
    emit('start');
}

const { isOwner, game } = defineProps({
    isOwner: {
        type: Boolean,
        default: false
    },
    game: {
        type: Object,
        required: true
    }
});

</script>
<template>
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
        <div v-if="isOwner">
            <ValidationButton msg="Start" @onClick="handleStartGame"/>
            <spacer :horizontal='40'/>
            <ValidationButton msg="Cancel Game" @onClick="handleLeave" color="red"/>
        </div>
        <div v-else>
            <ValidationButton msg="Leave" @onClick="handleLeave" color="red"/>
        </div>
    </WelcomeItem>
</template>

<style scoped>

</style>