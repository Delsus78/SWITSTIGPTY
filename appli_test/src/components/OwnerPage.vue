<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import {computed} from "vue";
import store from "@/store";
import Spacer from "@/components/Spacer.vue";
import StartIcon from "@/components/icons/IconStart.vue";

const emit = defineEmits(["leave","start"]);
const gameCode = computed(() => store.state.gameCode);
const playerNumber = computed(() => store.state.playerNumber);

const handleLeave = () => {
    emit('leave');
}

const handleStart = () => {
    emit('start');
}

</script>

<template>
    <WelcomeItem>
        <template #icon>
            <StartIcon />
        </template>
        <template #heading>Start whenever you want!</template>

        <p>Share this code with your friends so they can join your game.</p>
        <template #important>{{gameCode}}</template>
        <template #playerNumber>
            <div v-if="playerNumber !== null">
                {{playerNumber}} in lobby
            </div>
        </template>
        <ValidationButton msg="Start" @onClick="handleStart"/>
        <spacer :horizontal='40'/>
        <ValidationButton msg="leave" @onClick="handleLeave" color="red"/>
    </WelcomeItem>
</template>

<style scoped>
ValidationButton[msg="leave"] {
    margin-right: 10px; /* Vous pouvez ajuster cette valeur selon vos préférences */
}
</style>