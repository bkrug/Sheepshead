import { RouteComponentProps } from 'react-router';

export class IdUtils {
    public static getGameId(props: any) : string {
        var pathParts = props.location.pathname.split('/');
        var gameId = pathParts[pathParts.length - 1];
        return gameId;
    }

    public static setPlayerId(gameId: string, playerId: string) {
        window.localStorage.setItem('game' + gameId + 'player', playerId);
    }

    public static getPlayerId(gameId: string): string | null {
        return window.localStorage.getItem('game' + gameId + 'player');
    }

    public static setPlayerName(gameId: string, name: string) {
        window.localStorage.setItem('game' + gameId + 'name', name);
    }

    public static getPlayerName(gameId: string): string | null {
        return window.localStorage.getItem('game' + gameId + 'name');
    }
}