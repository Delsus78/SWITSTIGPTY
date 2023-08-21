import { createStore } from 'vuex';

export default createStore({
    state: {
        gameCode: null,
        playerNumber: 0,
        youtubeUrls: []
    },
    mutations: {
        setGameCode(state, value) {
            state.gameCode = value;
        },
        setPlayerNumber(state, value) {
            state.playerNumber = value;
        },
        setYoutubeUrls(state, value) {
            state.youtubeUrls = value;
        },
        clearAll(state) {
            state.gameCode = null;
            state.playerNumber = null;
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
        assignYoutubeUrls({ commit }, urls) {
            commit('setYoutubeUrls', urls);
        },
        resetAll({ commit }) {
            commit('clearAll');
        }
    }
});
