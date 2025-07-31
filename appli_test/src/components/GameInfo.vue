<template>
    <div class="game-info">
        <ul>
            <li><h2>Informations sur la partie</h2></li>
            <li>Joueurs :</li>
            <li><span class="number-info">{{ game.playersName }}</span></li>
            <li><span class="number-info">{{ game.numberOfManches }}</span> Manches</li>
            <li>Manche <span class="number-info">{{ game.currentManche }}</span> en cours</li>
            <li><span class="number-info">{{ game.pointPerRightVote }}</span> Points par vote sur l'imposteur</li>
            <li><span class="number-info">{{ game.pointPerVoteFooled }}</span> Points par vote trompé en tant qu'imposteur</li>
            <li v-if="game.isImpostorRevealedToHimself"><span class="number-info">L'imposteur sait qu'il est l'imposteur</span></li>
            <li v-if="!game.isImpostorRevealedToHimself"><span class="number-info">{{ game.pointForImpostorFoundHimself }}</span> Points si l'imposteur se trouve</li>
        </ul>
    </div>
</template>

<script>
export default {
    name: 'GameInfo',
    props: {
        game: {
            type: Object,
            required: true,
            default: () => ({
                gameCode: null,
                playersName: "",
                numberOfManches: 0,
                currentManche: 0,
                pointPerRightVote: 0,
                pointPerVoteFooled: 0,
                pointForImpostorFoundHimself: 0,
                isImpostorRevealedToHimself: false
            })
        }
    }
}
</script>

<style scoped>
.game-info {
    margin-top: 3rem;
    padding: 2rem;
    border: 2px solid var(--color-border);
    border-radius: 10px;
}

.game-info h2 {
    margin-top: 0;
}

.game-info ul {
    list-style-type: none;
    padding: 0;
}

.number-info {
    font-size: 1.4rem; /* un peu plus gros que le reste du texte */
    color: var(--vt-c-green-2); /* Utiliser une couleur contrastante, comme un vert vif par exemple */
    font-weight: bold; /* Pour rendre le texte gras */
    text-shadow: 0px 0px 5px rgba(0, 128, 0, 0.3); /* une petite ombre pour le faire ressortir davantage */
    margin-top: 10px;
}
</style>
