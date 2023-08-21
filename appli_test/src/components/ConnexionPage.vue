<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import TextBox from "@/components/TextBox.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import IconTooling from "@/components/icons/IconTooling.vue";
import {ref} from "vue";

const gameCode = ref('');

const emit = defineEmits(["code-retrieved","game-created"]);
const handleCodeValueChanged = (newValue) => {
    gameCode.value = newValue;
}

const handleCodeRetrieved = () => {
    emit('code-retrieved', gameCode.value);
}

const handleGameCreation = () => {
    emit('game-created');
}

</script>

<template>
    <WelcomeItem>
        <template #icon>
            <GameCodeIcon />
        </template>
        <template #heading>Join with Game Code</template>

        <TextBox placeholder="Enter game code here" @value-changed="handleCodeValueChanged"/>
        <ValidationButton @onClick="handleCodeRetrieved"/>
    </WelcomeItem>
    <WelcomeItem>
        <template #icon>
            <IconTooling />
        </template>
        <template #heading>Create a Game</template>

        <ValidationButton msg="Create" @onClick="handleGameCreation"/>
    </WelcomeItem>
</template>

<style scoped>

</style>