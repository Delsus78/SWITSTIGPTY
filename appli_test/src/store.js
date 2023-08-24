import { createStore } from 'vuex';

export default createStore({
    state: {
        gameCode: null,
        playerNumber: 0,
        players: [],
        youtubeUrls: [],
        votedId: null,
        playerId: null
    },
    mutations: {
        setGameCode(state, value) {
            state.gameCode = value;
        },
        setPlayerNumber(state, value) {
            state.playerNumber = value;
        },
        setPlayers(state, value) {
            state.players = value;
        },
        setYoutubeUrls(state, value) {
            state.youtubeUrls = value;
        },
        setPlayerId(state, value) {
            state.playerId = value;
        },
        setVotedId(state, value) {
            state.votedId = value;
        },
        SET_HAS_VOTED(state, playerId) {
            const player = state.players.find(p => p.id === playerId);
            if (player) player.hasVoted = true;
        },
        clearAll(state) {
            state.gameCode = null;
            state.playerNumber = null;
            state.players = null;
            state.youtubeUrls = [];
        }
    },
    actions: {
        assignGameCode({ commit }, code) {
            // Ici, ajoutez la logique si nécessaire
            commit('setGameCode', code);
        },
        assignPlayerNumber({ commit }, number) {
            commit('setPlayerNumber', number);
        },
        assignPlayers({ commit }, players) {
            commit('setPlayers', players);
        },
        assignYoutubeUrls({ commit }, urls) {
            commit('setYoutubeUrls', urls);
        },
        assignPlayerId({ commit }, id) {
            commit('setPlayerId', id);
        },
        assignVotedId({ commit }, id) {
            commit('setVotedId', id);
        },
        setHasVoted({ commit }, playerId) {
            commit('SET_HAS_VOTED', playerId);
        },
        resetAll({ commit }) {
            commit('clearAll');
        }
    }
});
