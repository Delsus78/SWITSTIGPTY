<template>
    <iframe width="560" height="315"
            :src="videoUrl"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowfullscreen>
    </iframe>
</template>

<script>
export default {
    props: {
        youtubeLink: {
            type: String,
            required: true
        }
    },
    computed: {
        videoId() {
            // Une simple logique pour extraire l'ID de la vidéo à partir de l'URL (ceci est basique et pourrait nécessiter d'être adapté selon la forme exacte de vos URL)
            return this.youtubeLink.split("youtu.be/")[1];
        },
        videoUrl() {
            return `https://www.youtube.com/embed/${this.videoId}?autoplay=1&mute=0&vq=small`;
        }
    }
};
</script>
