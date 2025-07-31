import { createStore } from 'vuex';

export default createStore({
    state: {
        game: {
            gameCode: null,
            playersName: 0,
            players: [],
            songsUrls: [],
            numberOfManches: 0,
            currentManche: 0,
            pointPerRightVote: 0,
            pointPerVoteFooled: 0,
            pointForImpostorFoundHimself: 0,
            isImpostorRevealedToHimself: false,
            gamePhase: "",
        },
        player: {
            id: "",
            name: "",
            imageUrl: "",
            score: 0,
            votedId: "",
            isOwner: false
        },
    },
    mutations: {
        // GAME MUTATIONS
        setGame(state, game) {
            state.game = game;
        },
        setGameCode(state, code) {
            state.game.gameCode = code;
        },
        setPlayersName(state, count) {
            state.game.playersName = count;
        },
        setPlayers(state, players) {
            state.game.players = players;
        },
        setSongsUrls(state, urls) {
            state.game.songsUrls = urls;
        },
        setNumberOfManches(state, number) {
            state.game.numberOfManches = number;
        },
        setCurrentManche(state, number) {
            state.game.currentManche = number;
        },
        setPointPerRightVote(state, point) {
            state.game.pointPerRightVote = point;
        },
        setPointPerVoteFooled(state, point) {
            state.game.pointPerVoteFooled = point;
        },
        setGamePhase(state, phase) {
            state.game.gamePhase = phase;
        },
        // PLAYER MUTATIONS
        setPlayer(state, player) {
            state.player = player;
        },
        setScore(state, score) {
            state.player.score = score;
        },
        setPlayerId(state, id) {
            state.player.id = id;
        },
        setVotedId(state, id) {
            state.player.votedId = id;
        },
        setPlayerName(state, name) {
            state.player.name = name;
        },
        setPlayerImageUrl(state, url) {
            state.player.imageUrl = url;
        },
        setPlayerIsOwner(state, isOwner) {
            state.player.isOwner = isOwner;
        },
        SET_HAS_VOTED(state, playerId) {
            const player = state.game.players.find(p => p.id === playerId);
            if (player) player.hasVoted = true;
        },
        clearAll(state) {
            state.game = {
                gameCode: null,
                playersName: 0,
                players: [],
                songsUrls: [],
                numberOfManches: 0,
                currentManche: 0,
                pointPerRightVote: 0,
                pointPerVoteFooled: 0,
                pointForImpostorFoundHimself: 0,
                isImpostorRevealedToHimself: false
            };
            state.player = {
                id: "",
                name: "",
                imageUrl: "",
                score: 0,
                votedId: "",
                isOwner: false
            };
        }
    },
    actions: {
        setHasVoted({ commit }, playerId) {
            commit('SET_HAS_VOTED', playerId);
        },
        resetAll({ commit }) {
            commit('clearAll');
        },
        // GAME ACTIONS
        setGame({ commit }, game) {
            commit('setGame', game);
        },
        setGameCode({ commit }, code) {
            commit('setGameCode', code);
        },
        setGamePhase({ commit }, phase) {
            commit('setGamePhase', phase);
        },
        setPlayersName({ commit }, count) {
            commit('setPlayersName', count);
        },
        setPlayers({ commit }, players) {
            commit('setPlayers', players);
        },
        setSongsUrls({ commit }, urls) {
            commit('setSongsUrls', urls);
        },
        setNumberOfManches({ commit }, number) {
            commit('setNumberOfManches', number);
        },
        setCurrentManche({ commit }, number) {
            commit('setCurrentManche', number);
        },
        setPointPerRightVote({ commit }, point) {
            commit('setPointPerRightVote', point);
        },
        setPointPerVoteFooled({ commit }, point) {
            commit('setPointPerVoteFooled', point);
        },
        // PLAYER ACTIONS
        setPlayer({ commit }, player) {
            commit('setPlayer', player);
        },
        setScore({ commit }, score) {
            commit('setScore', score);
        },
        setPlayerId({ commit }, id) {
            commit('setPlayerId', id);
        },
        setVotedId({ commit }, id) {
            commit('setVotedId', id);
        },
        setPlayerName({ commit }, name) {
            commit('setPlayerName', name);
        },
        setPlayerImageUrl({ commit }, url) {
            commit('setPlayerImageUrl', url);
        },
        setPlayerIsOwner({ commit }, isOwner) {
            commit('setPlayerIsOwner', isOwner);
        }
    }
});
