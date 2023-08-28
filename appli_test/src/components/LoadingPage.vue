<template>
    <div class="loading-container">
        <div class="spinner"></div>
    </div>
</template>

<script>
export default {
    name: 'Loading'
}
</script>

<style scoped>
.loading-container {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 100%;
}

.spinner {
    border: 16px solid var(--vt-c-green-1); /* Vert clair */
    border-top: 16px solid var(--vt-c-green-2); /* Vert vif */
    border-radius: 50%;
    width: 5rem;
    height: 5rem;
    animation: spin 0.3s linear infinite;
    /* shadow */
    box-shadow: 0 0 1rem var(--vt-c-green-dark);
}

@keyframes spin {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
}
</style>
