<template>
    <button class="validation-button" @click="validate">
        Valider
    </button>
</template>

<script>
export default {
    methods: {
        validate() {
            // Logique de validation ici
            console.log("Bouton validé !");
        }
    }
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
    background-color: green;
}

.validation-button:hover {
    background-color: darkgreen;
}
</style>
