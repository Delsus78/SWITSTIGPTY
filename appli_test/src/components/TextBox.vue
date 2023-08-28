<template>
    <div :class="{ 'text-box': true, 'errored': isErrored }">
        <input :type="inputType" v-model="value" :placeholder="placeholder" @input="handleInput" />
    </div>
</template>

<script setup>
import {computed, ref, watch} from 'vue';

const { placeholder, isErrored, defaultValue, isNumberOnly } = defineProps({
    placeholder: {
        type: String,
        required: true
    },
    isErrored: {
        type: Boolean,
        required: false,
        default: false
    },
    defaultValue: {
        type: [String, Number],
        required: false,
        default: ''
    },
    isNumberOnly: {
        type: Boolean,
        required: false,
        default: false
    }
});

const emit = defineEmits(["value-changed"]);
const inputType = computed(() => (isNumberOnly ? 'number' : 'text'));
const value = ref(defaultValue);

watch(value, (newValue) => {
    emit('value-changed', newValue);
});

const handleInput = (event) => {
    if (isNumberOnly && /[^0-9]/.test(event.target.value)) {
        event.target.value = event.target.value.replace(/[^0-9]/g, '');
        value.value = event.target.value;
    }
};
</script>

<style scoped>
.text-box {
    position: relative;
    margin-bottom: 1rem;
    border: 1px solid var(--color-border);
    border-radius: 4px;
    cursor: pointer;
}

.text-box.errored {
    border-color: var(--vt-c-red-2);
}

input {
    padding: 0.5rem 1rem;
    border-radius: 4px;
    background: var(--color-background);
    color: var(--color-text);
    border: 1px solid var(--color-border);
    font-size: 1rem;
    width: 100%;
    appearance: none;
    cursor: pointer;
    outline: none;
    transition: border-color 0.3s, box-shadow 0.3s;
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

input:hover {
    border-color: var(--vt-c-green-2);
}

input:focus {
    border-color: var(--vt-c-green-2);
    box-shadow: 0 0 0 3px rgba(0, 128, 0, 0.3); /* Cette couleur est juste un exemple. */
}

</style>
