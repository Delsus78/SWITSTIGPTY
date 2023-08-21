<template>
    <span :style="{ display: 'inline-block', height: vertical + 'px', width: horizontal + 'px' }"></span>
</template>


<script>
export default {
    props: {
        vertical: {
            type: Number,
            default: 0
        },
        horizontal: {
            type: Number,
            default: 0
        }
    }
}
</script>
