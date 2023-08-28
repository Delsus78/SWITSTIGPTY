<script setup>

import WaitIcon from "@/components/icons/IconWait.vue";
import StartIcon from "@/components/icons/IconStart.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import EmbedYoutube from "@/components/EmbedYoutube.vue";
import WelcomeItem from "@/components/WelcomeItem.vue";
import VotePage from "@/components/VotePage.vue";

const emit = defineEmits(["endRound","vote"]);

const handleEndRound = async () => {
    emit('endRound');
}

const handleVote = async (playerId) => {
    emit('vote', playerId);
}

const { isOwner, youtubeUrl, players } = defineProps({
    isOwner: {
        type: Boolean,
        default: false
    },
    youtubeUrl: {
        type: String,
        default: ""
    },
    players: {
        type: Array,
        default: []
    }
});

</script>

<template>
    <WelcomeItem>
        <template #heading>GAME TIME</template>
        <template #icon>
            <StartIcon />
        </template>
        <EmbedYoutube :youtube-link="youtubeUrl"></EmbedYoutube>
        <ValidationButton v-if="isOwner" msg="STOOOOP" @onClick="handleEndRound" color="red"/>
    </WelcomeItem>

    <WelcomeItem>
        <template #heading>Vote</template>
        <template #icon>
            <WaitIcon />
        </template>
        <VotePage @vote="handleVote" :items="players"></VotePage>
    </WelcomeItem>
</template>

<style scoped>

</style>