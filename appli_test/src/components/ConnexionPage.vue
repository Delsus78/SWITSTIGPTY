<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import TextBox from "@/components/TextBox.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import IconTooling from "@/components/icons/IconTooling.vue";
import {ref} from "vue";
import PseudoIcon from "@/components/icons/IconPseudo.vue";

const gameCode = ref('');
const type = ref(localStorage.getItem('type') || 'all');
const genre = ref(localStorage.getItem('genre') || 'rap');
const numberOfManches = ref(+localStorage.getItem('numberOfManches') || 1);
const pointsPerRightVote = ref(+localStorage.getItem('pointsPerRightVote') || 1);
const pointsPerVoteFooled = ref(+localStorage.getItem('pointsPerVoteFooled') || 2);
const pseudo = ref(localStorage.getItem('pseudo') || '');
const isErrored = ref(false);
const emit = defineEmits(["code-retrieved","game-created"]);

const saveLocalStorage = () =>  {
    localStorage.setItem('pseudo', pseudo.value);
    localStorage.setItem('type', type.value);
    localStorage.setItem('genre', genre.value);
    localStorage.setItem('numberOfManches', numberOfManches.value.toString());
    localStorage.setItem('pointsPerRightVote', pointsPerRightVote.value.toString());
}

const handleCodeValueChanged = (newValue) => {
    gameCode.value = newValue;
}

const handlePseudoValueChanged = (newValue) => {
    pseudo.value = newValue;
}

const handleCodeRetrieved = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();
    emit('code-retrieved', gameCode.value, pseudo.value);
}

const handleGameCreation = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();

    emit('game-created',
        {
            type: type.value,
            genre: genre.value,
            numberOfManches: numberOfManches.value,
            pointsPerRightVote: pointsPerRightVote.value,
            pointsPerVoteFooled: pointsPerVoteFooled.value
        }, pseudo.value);
}

</script>

<template>
    <WelcomeItem>
        <template #icon>
            <PseudoIcon />
        </template>
        <template #heading>Super pseudo</template>

        <TextBox :default-value="pseudo" placeholder="PSEUDO" @value-changed="handlePseudoValueChanged" :is-errored="isErrored"/>
    </WelcomeItem>
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

        <div class="game-settings">
            <div class="number-inputs-container">
                <h4>Number of rounds</h4>
                <h4>Points / right vote</h4>
                <h4>Points / fooled vote</h4>
            </div>
            <div class="number-inputs-container">
                <text-box :default-value="numberOfManches" placeholder="Number of rounds" is-number-only @value-changed="numberOfManches = $event" />
                <text-box :default-value="pointsPerRightVote" placeholder="Points/ right vote" is-number-only @value-changed="pointsPerRightVote = $event" />
                <text-box :default-value="pointsPerVoteFooled" placeholder="Points/ fooled vote" is-number-only @value-changed="pointsPerVoteFooled = $event" />
            </div>
        </div>
        <ValidationButton msg="Create" @onClick="handleGameCreation"/>

    </WelcomeItem>
</template>

<style scoped>
.number-inputs-container {
    display: flex;
    justify-content: space-between;
    gap: 1rem; /* Espacement entre les éléments */
}
</style>