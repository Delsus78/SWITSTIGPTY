<script setup>
import GridItem from './GridItem.vue';
import {ref} from "vue";

const { items } = defineProps({
    items: {
        type: Array,
        default: () => []
    }
});
const emit = defineEmits(["vote"]);
const buttonsOn = ref(true);
const handleVote = (itemId) => {
    emit('vote', itemId);

    buttonsOn.value = false;
    console.log("Button " + buttonsOn.value);
}
</script>

<template>
    <div class="grid-list">
        <GridItem
                v-for="item in items"
                :itemId="item.id"
                :title="item.name"
                :imageUrl="item.imageUrl"
                :has-voted="item.hasVoted"
                :is-button-on="buttonsOn.value"
                @vote="handleVote"
        />
    </div>
</template>

<style scoped>
.grid-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 1rem;
}
</style>