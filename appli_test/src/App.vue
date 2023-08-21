<script setup>
import HelloWorld from './components/HelloWorld.vue'
import ConnexionPage from './components/ConnexionPage.vue'
import Logo from "@/components/icons/Logo.vue";
</script>

<template>
  <header>
    <Logo class="logo" />

    <div class="wrapper">
      <HelloWorld msg="SWITSTIGPTY" />
    </div>
  </header>

  <main>
    <ConnexionPage />
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
  color: var(--color-logo);
  width: 125px;
  height: 125px;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
    color: var(--color-logo);
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
