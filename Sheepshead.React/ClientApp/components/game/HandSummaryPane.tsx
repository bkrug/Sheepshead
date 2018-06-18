import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import { PlayState, TrickChoice, CardSummary, GameScore, HandScores } from './PlayState';

export interface HandSummaryPaneState {
    gameId: string;
    playerId: string;
    points: GameScore[];
    coins: GameScore[];
}

export interface HandSummaryPaneProps extends React.Props<any> {
    gameId: string;
}

export default class HandSummaryPane extends React.Component<HandSummaryPaneProps, HandSummaryPaneState> {
    displayInterval: number;

    constructor(props: HandSummaryPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            points: [],
            coins: []
        };
        var self = this;
        FetchUtils.get(
            'Game/HandSummary?gameId=' + this.state.gameId,
            function (json: any): void {
                var pointsArray: GameScore[] = [];
                for (var key in json.points)
                    pointsArray.push({ name: key, score: json.points[key] });

                var coinsArray: GameScore[] = [];
                for (var key in json.coins)
                    coinsArray.push({ name: key, score: json.coins[key] });

                self.setState({
                    coins: coinsArray,
                    points: pointsArray
                });
            }
        );
    }

    public render() {
        var pointList = this.state.points.map((score: GameScore, i: number) =>
            <div key={i}>
                {score.name}: {score.score || '-'}
            </div>);

        var coinList = this.state.coins.map((score: GameScore, i: number) =>
            <div key={i} style={{ display: 'inline-flex', margin: '0px 20px', textAlign: 'center' }}>
                {score.name}
                <br />
                {score.score || '-'}
            </div>);

        return (
            <div>
                <div>
                    <h4>Points from this Hand</h4>
                    {pointList}
                </div>
                <div>
                    <h4>Coins from this Hand</h4>
                    {coinList}
                </div>
            </div>
        );
    }
}