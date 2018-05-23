import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import GameDetails from './GameDetails';
import HandDetails from './HandDetails';
import ActionPane from './ActionPane';
import CardsHeldPane from './CardsHeldPane';

export interface PlayPaneState {
    gameId: string;
}

export class PlayPane extends React.Component<RouteComponentProps<{}>, PlayPaneState> {
    constructor(props: any) {
        super(props);
        this.state = { gameId: IdUtils.getGameId(props) };
    }

    public render() {
        return (
            <div className="playPane">
                <GameDetails gameId={this.state.gameId} />
                <HandDetails />
                <ActionPane />
                <CardsHeldPane />
            </div>
        );
    }
}