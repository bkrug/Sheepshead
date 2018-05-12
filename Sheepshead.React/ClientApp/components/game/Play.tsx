import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface PlayState {
    gameId: string;
}

export class Play extends React.Component<RouteComponentProps<{}>, PlayState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: this.getGameId(props) };
    }

    private getGameId(props: any) {
        var pathParts = props.location.pathname.split('/');
        var indexOfGameId = pathParts.indexOf('Play') + 1;
        var gameId = pathParts[indexOfGameId];
        return gameId;
    }

    public render() {
        return (
            <div>
                <h4>Play Sheepshead</h4>
                Waiting for other players.
            </div>
        );
    }
}