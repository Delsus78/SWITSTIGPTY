<template>
    <div class="dropdown-container" @click="toggleDropdown">
        <div :class="{ 'selected-value': true, 'focused': isOpen }">
            <span v-if="!searchable || !isOpen">{{ selectedValue }}</span>
            <input v-if="searchable" v-model="searchTerm" type="text" placeholder="Recherche...">
        </div>
        <ul v-if="isOpen" class="dropdown-list">
            <li v-for="option in filteredOptions" :key="option.value" @click="selectOption(option)">
                {{ option.label }}
            </li>
        </ul>
    </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue';

const { options, defaultOption, searchable } = defineProps({
    options: {
        type: Array,
        required: true
    },
    defaultOption: {
        type: String,
        default: ''
    },
    searchable: {
        type: Boolean,
        default: false
    }
});

const searchTerm = ref('');

const filteredOptions = computed(() => {
    if (!searchTerm.value) return options;
    return options.filter(option => option.label.toLowerCase().includes(searchTerm.value.toLowerCase()));
});

const emit = defineEmits(["value-changed"]);

const isOpen = ref(false);
const selectedValue = ref(defaultOption || options[0]?.label || '');

const toggleDropdown = () => {
    isOpen.value = !isOpen.value;
}

const selectOption = (option) => {
    selectedValue.value = option.label;
    emit('value-changed', option.value);
    isOpen.value = false;
}

const handleClickOutside = (event) => {
    if (!event.target.closest('.dropdown-container')) {
        isOpen.value = false;
    }
}

onMounted(() => {
    document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
    document.removeEventListener('click', handleClickOutside);
});
</script>

<style scoped>
.dropdown-container {
    position: relative;
    margin-bottom: 1rem;
    border: 1px solid var(--color-border);
    border-radius: 4px;
    cursor: pointer;
}

.selected-value {
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

.selected-value:hover {
    border-color: var(--vt-c-green-2);
}

.focused {
    border-color: var(--vt-c-green-2);
    box-shadow: 0 0 0 3px rgba(0, 128, 0, 0.3); /* Cette couleur est juste un exemple. */
}

.dropdown-list {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: var(--color-background);
    border: 1px solid var(--color-border);
    border-top: none;
    border-radius: 0 0 4px 4px;
    list-style: none;
    padding: 0;
    margin: 0;
    z-index: 1000;
}

.dropdown-list li {
    padding: 0.5rem 1rem;
    cursor: pointer;
}

.dropdown-list li:hover {
    background: var(--vt-c-green-2);
    color: white;
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