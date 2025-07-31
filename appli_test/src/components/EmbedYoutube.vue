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
            console.log("youtubeLink", this.youtubeLink);
            return this.youtubeLink.split("https://www.youtube.com/watch?v=")[1];
        },
        videoUrl() {
          // start the music directly at 30 seconds
          return `https://www.youtube.com/embed/${this.videoId}?autoplay=1&mute=0&vq=small&t=30`;
        }
    }
};
</script>
