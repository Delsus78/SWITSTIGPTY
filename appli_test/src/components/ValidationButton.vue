<template>
    <button :class="['validation-button', color]" @click="validate">
        {{ msg ? msg : "Validate" }}
    </button>
</template>

<script setup>

const { msg, color } = defineProps({
    msg: {
        type: String,
        required: false
    },
    color: {
        type: String,
        default: 'green'
    }
});


const emit = defineEmits(["onClick"]);

const validate = () => {
    // Logique de validation ici
    emit('onClick');
}
</script>

<style scoped>
.validation-button {
    /* styles communs pour tous les boutons */
    margin-top: 1rem;
    padding: 0.5rem 1rem;
    border: none;
    border-radius: 4px;
    color: white;
    cursor: pointer;
    font-size: 1rem;
    transition: background-color 0.3s;
}

.green {
    background-color: var(--vt-c-green-2);
}
.green:hover {
    background-color: var(--vt-c-green-dark);
}

/* Ajoutez des styles pour d'autres couleurs si nécessaire */
.blue {
    background-color: var(--vt-c-blue-1);
}
.blue:hover {
    background-color: var(--vt-c-blue-dark);
}

.red {
    background-color: var(--vt-c-red-1);
}
.red:hover {
    background-color: var(--vt-c-red-dark);
}

</style>
