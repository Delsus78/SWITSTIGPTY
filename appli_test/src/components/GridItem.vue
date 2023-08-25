<template>
    <div class="grid-item">
        <div :class="{ 'image-container': true, 'has-voted-border': hasVoted }">
            <img :src="imageUrl" alt="image" width="100" height="100">
        </div>
        <svg v-if="hasVoted" class="vote-text-path" viewBox="0 0 50 50" xmlns="http://www.w3.org/2000/svg">
            <path id="curve" d="M 50, 100 a 50,50 0 1,0 100,0" fill="transparent"/>
            <text>
                <textPath xlink:href="#curve" startOffset="50%">
                    Confirmed vote!
                </textPath>
            </text>
        </svg>
        <h3>{{ title }}</h3>
        <ValidationButton v-if="isButtonOn" @onClick="handleVote" msg="Vote">Vote</ValidationButton>
        <div v-else class="button-placeholder"></div>
    </div>
</template>

<script setup>
import ValidationButton from "@/components/ValidationButton.vue";

const { title, itemId, imageUrl, hasVoted, isButtonOn } = defineProps({
    isButtonOn: {
        type: Boolean,
        required: true,
        default: true
    },
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
    position: relative; /* Ajouté pour être un point de référence pour le positionnement absolu de l'enfant */
}

.image-container {
    display: inline-block;
    border: 3px solid var(--vt-c-red-1);
    border-radius: 50%;
}

.image-container img {
    border-radius: 45%;
    border: 1px solid black;
}

.image-container.has-voted-border {
    border: 3px solid var(--vt-c-green-1);
}

.vote-text-path {
    width: auto;
    height: 4rem;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-200%, -270%); /* Utilisez translate pour centrer sur l'image */
    overflow: visible;
}

.vote-text-path text {
    font-size: larger;
    fill: var(--vt-c-green-1);
    text-anchor: middle;  /* pour centrer le texte sur le chemin */
}

.button-placeholder {
    width: 4rem;
    height: 3rem;
    visibility: hidden;
}
</style>

