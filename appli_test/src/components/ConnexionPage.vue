<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import TextBox from "@/components/TextBox.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import IconTooling from "@/components/icons/IconTooling.vue";
import {ref} from "vue";
import PseudoIcon from "@/components/icons/IconPseudo.vue";

const gameCode = ref('');

const numberOfManches = ref(+localStorage.getItem('numberOfManches') || 1);
const pointsPerRightVote = ref(+localStorage.getItem('pointsPerRightVote') || 1);
const pointsPerVoteFooled = ref(+localStorage.getItem('pointsPerVoteFooled') || 2);
const pointsForImpostorFoundHimself = ref(+localStorage.getItem('pointsForImpostorFoundHimself') || 2);
const isImpostorRevealedToHimself = ref(localStorage.getItem('isImpostorRevealedToHimself') === 'true' || false);

const pseudo = ref(localStorage.getItem('pseudo') || '');
const isErrored = ref(false);
const emit = defineEmits(["code-retrieved","game-created"]);

const saveLocalStorage = () =>  {
    localStorage.setItem('pseudo', pseudo.value);
    localStorage.setItem('numberOfManches', numberOfManches.value.toString());
    localStorage.setItem('pointsPerRightVote', pointsPerRightVote.value.toString());
    localStorage.setItem('pointsPerVoteFooled', pointsPerVoteFooled.value.toString());
    localStorage.setItem('pointsForImpostorFoundHimself', pointsForImpostorFoundHimself.value.toString());
    localStorage.setItem('isImpostorRevealedToHimself', isImpostorRevealedToHimself.value.toString());
}

const handleCodeValueChanged = (newValue) => {
    gameCode.value = newValue;
}

const handlePseudoValueChanged = (newValue) => {
    pseudo.value = newValue;
}

const handleCodeRetrieved = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();
    emit('code-retrieved', gameCode.value, pseudo.value);
}

const handleGameCreation = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();

    emit('game-created',
        {
            numberOfManches: numberOfManches.value,
            pointsPerRightVote: pointsPerRightVote.value,
            pointsPerVoteFooled: pointsPerVoteFooled.value,
            pointsForImpostorFoundHimself: pointsForImpostorFoundHimself.value,
            isImpostorRevealedToHimself: isImpostorRevealedToHimself.value
        }, pseudo.value);
}

</script>

<template>
  <div class="connexion-page">
    <div class="left-panel">
      <WelcomeItem>
        <template #icon>
          <PseudoIcon />
        </template>
        <template #heading>Super pseudo</template>
        <TextBox :default-value="pseudo" placeholder="PSEUDO" @value-changed="handlePseudoValueChanged" :is-errored="isErrored"/>
      </WelcomeItem>
      <WelcomeItem>
        <template #icon>
          <GameCodeIcon />
        </template>
        <template #heading>Rejoindre une partie</template>
        <TextBox placeholder="Code de la partie" @value-changed="handleCodeValueChanged"/>
        <ValidationButton msg="Rejoindre" @onClick="handleCodeRetrieved"/>
      </WelcomeItem>
    </div>
    <div class="right-panel">
      <WelcomeItem>
        <template #icon>
          <IconTooling />
        </template>
        <template #heading>Créer une partie</template>
        <div class="game-settings">
          <div class="number-inputs-container">
            <div class="input-group">
              <label>Nombre de manches</label>
              <text-box :default-value="numberOfManches" placeholder="Nombre de manches" is-number-only @value-changed="numberOfManches = $event" />
            </div>
            <div class="input-group">
              <label>Points par vote réussi</label>
              <text-box :default-value="pointsPerRightVote" placeholder="Points/vote réussi" is-number-only @value-changed="pointsPerRightVote = $event" />
            </div>
            <div class="input-group">
              <label>Points par vote trompé</label>
              <text-box :default-value="pointsPerVoteFooled" placeholder="Points/vote trompé" is-number-only @value-changed="pointsPerVoteFooled = $event" />
            </div>
            <div class="input-group" v-if="!isImpostorRevealedToHimself">
              <label>Points si l'imposteur se trouve</label>
              <text-box :default-value="pointsForImpostorFoundHimself" placeholder="Points/imposteur trouvé" is-number-only @value-changed="pointsForImpostorFoundHimself = $event" />
            </div>
            <div class="input-group">
              <label>Révéler l'imposteur à lui-même ?</label>
              <label class="switch">
                <input type="checkbox" v-model="isImpostorRevealedToHimself" />
                <span class="slider round"></span>
              </label>
            </div>
          </div>
        </div>
        <ValidationButton msg="Créer" @onClick="handleGameCreation"/>
      </WelcomeItem>
    </div>
  </div>
</template>

<style scoped>
.connexion-page {
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: flex-start;
  gap: 3rem;
  padding: 2rem 0;
}

.left-panel, .right-panel {
  border-radius: 1rem;
  box-shadow: 0 2px 12px rgba(0,0,0,0.08);
  padding: 2rem 2.5rem;
  min-width: 350px;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.game-settings {
  margin-top: 1rem;
}

.number-inputs-container {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

label {
  font-size: 0.95rem;
  color: #444;
  margin-bottom: 0.2rem;
}

/* Switch/toggle style */
.switch {
  position: relative;
  display: inline-block;
  width: 44px;
  height: 24px;
}
.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}
.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: .4s;
  border-radius: 24px;
}
.slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  transition: .4s;
  border-radius: 50%;
}
input:checked + .slider {
  background-color: #42b983;
}
input:checked + .slider:before {
  transform: translateX(20px);
}
</style>