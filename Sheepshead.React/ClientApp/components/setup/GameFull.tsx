import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface GameFullState {
    gameId: string;
}

export class GameFull extends React.Component<RouteComponentProps<{}>, GameFullState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: this.getGameId(props) };
    }

    private getGameId(props: any) {
        var pathParts = props.location.pathname.split('/');
        var indexOfGameId = pathParts.indexOf('GameFull') + 1;
        var gameId = pathParts[indexOfGameId];
        return gameId;
    }

    public render() {
        return (
            <div>
                <h4>Game Full</h4>
                Sorry, all slots for human players have been filled.
            </div>
        );
    }
}