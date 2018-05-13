import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface GameFullState {
    gameId: string;
}

export class GameFull extends React.Component<RouteComponentProps<{}>, GameFullState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: IdUtils.getGameId(props) };
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