<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import TextBox from "@/components/TextBox.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import IconTooling from "@/components/icons/IconTooling.vue";
import {onMounted, ref} from "vue";
import Dropdown from "@/components/Dropdown.vue";
import axios from "axios";
import config from "@/config";
import PseudoIcon from "@/components/icons/IconPseudo.vue";

const gameCode = ref('');
const type = ref('all');
const genre = ref('rap');
const pseudo = ref('');
const isErrored = ref(false);
const emit = defineEmits(["code-retrieved","game-created"]);

const allGenres = ref([]);

const getAllGenres = async () => {
    const response = await axios.get(config.apiUrl + "Game/allgenres");
    let res = response.data.map(genre => ({value: genre, label: genre}));
    return res;
}

onMounted(async () => {
    allGenres.value = await getAllGenres();
});

const handleCodeValueChanged = (newValue) => {
    gameCode.value = newValue;
}

const handleTypeValueChanged = (newValue) => {
    type.value = newValue;
}

const handlePseudoValueChanged = (newValue) => {
    pseudo.value = newValue;
}

const handleGenreValueChanged = (newValue) => {
    genre.value = newValue;
}

const handleCodeRetrieved = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }

    emit('code-retrieved', gameCode.value, pseudo.value);
}

const handleGameCreation = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }

    emit('game-created', {type: type.value, genre: genre.value}, pseudo.value);
}

</script>

<template>
    <WelcomeItem>
        <template #icon>
            <PseudoIcon />
        </template>
        <template #heading>Super pseudo</template>

        <TextBox placeholder="PSEUDO" @value-changed="handlePseudoValueChanged" :is-errored="isErrored"/>
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

        <ValidationButton msg="Create" @onClick="handleGameCreation"/>
        <Dropdown default-option='all' :options="[{ value: 'all', label: 'All' }, { value: 'top-all-time', label: 'Top All Time' }, { value: 'genre', label: 'Genre' }]" @value-changed="handleTypeValueChanged" />
        <Dropdown v-if="type === 'genre'" default-option='rap' :options="allGenres" :searchable="true" @value-changed="handleGenreValueChanged" />

    </WelcomeItem>
</template>

<style scoped>

</style>