import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface PlayState {
    gameId: string;
}

export class Play extends React.Component<RouteComponentProps<{}>, PlayState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: IdUtils.getGameId(props) };
        this.getUrl = this.getUrl.bind(this);
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