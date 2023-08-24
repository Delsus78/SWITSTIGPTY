<template>
    <div class="grid-item">
        <img :src="imageUrl" alt="image" width="100" height="100">
        <h3>{{ title }}</h3>
        <h4>{{ hasVoted ? "Confirmed vote !" : "Hasn't voted yet"}}</h4>
        <ValidationButton v-if="isButtonOn" @onClick="handleVote" msg="Vote">Vote</ValidationButton>
    </div>
</template>

<script setup>
import ValidationButton from "@/components/ValidationButton.vue";

const { title, itemId, imageUrl, hasVoted } = defineProps({
    title: {
        type: String,
        required: true
    },
    itemId: {
        type: String,
        required: true
    },
    imageUrl: {
        type: String,
        required: false
    },
    hasVoted: {
        type: Boolean,
        required: false,
        default: false
    },
    isButtonOn: {
        type: Boolean,
        required: true,
        default: true
    }
});
const emit = defineEmits(["vote"]);

const handleVote = () => {
    emit('vote', itemId);
}
</script>

<style scoped>
.grid-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
}

.grid-item img {
    border-radius: 50%;
    border: 1px solid black;
}


</style>
