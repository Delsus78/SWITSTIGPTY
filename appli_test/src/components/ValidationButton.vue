<template>
    <button class="validation-button" @click="validate">
        {{ msg ? msg : "Validate" }}
    </button>
</template>

<script setup>
import { ref } from 'vue';

const { msg } = defineProps({
    msg: {
        type: String,
        required: false
    }
});

const validate = () => {
    // Logique de validation ici
    console.log("Bouton validé !");
}
</script>

<style scoped>
.validation-button {
    margin-top: 1rem;
    padding: 0.5rem 1rem;
    border: none;
    border-radius: 4px;
    color: white;
    cursor: pointer;
    font-size: 1rem;
    transition: background-color 0.3s;
    background-color: var(--vt-c-green-2);
}

.validation-button:hover {
    background-color: var(--vt-c-green-dark);
}
</style>
