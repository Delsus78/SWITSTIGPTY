<script setup>
import GridItem from './GridItem.vue';

const { items } = defineProps({
    items: {
        type: Array,
        default: () => []
    }
});

const emit = defineEmits(["vote"]);

const handleVote = (itemId) => {
    items.forEach(item => {
        item.isButtonOn = false;
    });
    emit('vote', itemId);

}
</script>

<template>
    <div class="grid-list">
        <GridItem
            v-for="item in items"
            :key="item.id"
            :itemId="item.id"
            :title="item.name"
            :imageUrl="item.imageUrl"
            :has-voted="item.hasVoted"
            :is-button-on="item.isButtonOn"
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