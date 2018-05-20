import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

export interface GameDetailsState {
    gameId: string;
}

export default class GameDetails extends React.Component<any, any> {
    constructor(props: GameDetailsState) {
        super(props);
        //this.state = { gameId: IdUtils.getGameId(props) };
    }

    public render() {
        return (
            <div className="gameDetails">
                <h4>Game Details</h4>
            </div>
        );
    }
}