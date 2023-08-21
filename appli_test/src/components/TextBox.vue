<template>
    <div class="text-box">
        <input type="text" v-model="password" :placeholder="placeholder" />
    </div>
</template>

<script setup>
import { ref } from 'vue';

const { placeholder } = defineProps({
    placeholder: {
        type: String,
        required: true
    }
});

const password = ref('');
</script>

<style scoped>
.text-box {
    margin-top: 1rem;
    display: flex;
}

input[type="text"] {
    padding: 0.5rem 1rem;
    border-radius: 4px;
    background: var(--color-background);
    color: var(--color-text);
    border: 1px solid var(--color-border);
    font-size: 1rem;
    width: 100%;
}
</style>
