import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

export interface GameScore {
    name: string;
    score: number;
}

export interface GameDetailsState {
    gameId: string;
    scores: GameScore[];
}

export default class GameDetails extends React.Component<any, any> {
    constructor(props: GameDetailsState) {
        super(props);
        var scores: GameScore[] = [];
        this.state = {
            gameId: props.gameId,
            scores: scores
        };
        var self = this;
        FetchUtils.get(
            'Game/Summary?gameId=' + this.state.gameId,
            function(json: GameScore[]): void {
                self.setState({ scores: json });
            }
        );
    }


    public render() {
        var scoreList = this.state.scores.map((score: GameScore) => 
                <div>{score.name} - {score.score}</div>
            );
        return (
            <div className="gameDetails">
                <h4>Game Details</h4>
                {scoreList}
            </div>
        );
    }
}