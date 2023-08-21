<template>
    <div>
        <iframe :src="embedLink" width="560" height="315" allowfullscreen></iframe>
    </div>
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
        embedLink() {
            return this.youtubeLink.replace('https://youtu.be/', 'https://www.youtube.com/embed/');
        }
    }
}
</script>
