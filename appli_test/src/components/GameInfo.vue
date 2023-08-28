<template>
    <div class="game-info">
        <ul>
            <li><span class="number-info">{{ game.playerCount }}</span> Joueurs</li>
            <li><span class="number-info">{{ game.numberOfManches }}</span> Manches</li>
            <li>Manche <span class="number-info">{{ game.currentManche }}</span> en cours</li>
            <li><span class="number-info">{{ game.pointPerRightVote }}</span> Points par vote sur l'imposteur</li>
            <li><span class="number-info">{{ game.pointPerVoteFooled }}</span> Points par vote trompé en tant qu'imposteur</li>
            <li v-if="game.genre">Genre : <span class="number-info">{{ game.genre }}</span></li>
            <li>Type : <span class="number-info">{{ game.type }}</span></li>
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
                playerCount: 0,
                numberOfManches: 0,
                currentManche: 0,
                pointPerRightVote: 0,
                pointPerVoteFooled: 0,
                genre: "",
                type: ""
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
