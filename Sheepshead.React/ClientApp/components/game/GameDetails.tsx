import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';

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
        fetch('Game/Summary?gameId=' + this.state.gameId, {
            method: 'GET'
        }).then(function (response) {
            return response.json();
        }).then(function (json) {
            self.setState({ scores: json });
        }).catch(function (ex) {
            console.log('parsing failed', ex)
        });
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