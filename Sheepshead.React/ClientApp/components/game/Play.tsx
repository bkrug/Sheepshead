import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface PlayState {
    gameId: string;
}

export class Play extends React.Component<RouteComponentProps<{}>, PlayState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: this.getGameId(props) };
        this.getUrl = this.getUrl.bind(this);
    }

    private getGameId(props: any) {
        var pathParts = props.location.pathname.split('/');
        var gameId = pathParts[pathParts.length-1];
        return gameId;
    }

    private getUrl() {
        return window.location.host + '/setup/RegisterHuman/' + this.state.gameId;
    }

    public render() {
        return (
            <div>
                <h4>Play Sheepshead</h4>
                <p>Waiting for other players.</p>
                <p>Share this url with friends that you will play with: <b>{this.getUrl()}</b> </p>
            </div>
        );
    }
}