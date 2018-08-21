import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import { PlayState, TrickChoice, CardSummary, GameScore, HandScores, HandSummary } from './PlayState';
import Card from './Card';

export interface HandSummaryPaneState {
    gameId: string;
    playerId: string;
    points: GameScore[];
    coins: GameScore[];
    mustRedeal: boolean;
    tricks: { key: string, value: CardSummary[] }[],
    showGroupedTricks: boolean;
}

export interface HandSummaryPaneProps extends React.Props<any> {
    gameId: string;
    onSummaryPhaseComplete: () => void;
}

export default class HandSummaryPane extends React.Component<HandSummaryPaneProps, HandSummaryPaneState> {
    displayInterval: number;

    constructor(props: HandSummaryPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            points: [],
            coins: [],
            mustRedeal: false,
            tricks: [],
            showGroupedTricks: false
        };
        var self = this;
        this.showGroupedTricks = this.showGroupedTricks.bind(this);
        this.hideGroupedTricks = this.hideGroupedTricks.bind(this);
        this.renderModal = this.renderModal.bind(this);

        FetchUtils.get(
            'Game/HandSummary?gameId=' + this.state.gameId,
            function (json: HandSummary): void {
                var pointsArray: GameScore[] = [];
                for (var key in json.points)
                    pointsArray.push({ name: key, score: json.points[key] });

                var coinsArray: GameScore[] = [];
                for (var key in json.coins)
                    coinsArray.push({ name: key, score: json.coins[key] });

                self.setState({
                    coins: coinsArray,
                    points: pointsArray,
                    mustRedeal: json.mustRedeal,
                    tricks: json.tricks
                });
            }
        );
    }

    private showGroupedTricks() : void {
        this.setState({
            showGroupedTricks: true
        });
    }

    private hideGroupedTricks() : void {
        this.setState({
            showGroupedTricks: false
        });        
    }

    private renderModal() {
        var self = this;
        var playerList = this.state.tricks.map((trick: { key: string, value: CardSummary[] }, i: number) =>
            <div key={i} className='trickSummary'>
                <p>{trick.key}</p>
                {trick.value.map((cardSummary: CardSummary, j: number) =>
                    <p className={cardSummary.abbreviation.indexOf('♥') >= 0 || cardSummary.abbreviation.indexOf('♦') >= 0 ? 'redCard' : 'blkCard'}>{cardSummary.abbreviation}</p>
                )}
            </div>
        );
        return (
            <div className="modalDialog">
                <div>
                    {playerList}
                </div>
            </div>
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
                { this.state.mustRedeal
                    ? <h3>Must re-deal. There was no picker. </h3>
                    : <div>
                        <div onMouseOver={this.showGroupedTricks} onMouseOut={this.hideGroupedTricks}>
                            <h4>Points from this Hand</h4>
                            {pointList}
                        </div>
                        <div>
                            <h4>Coins from this Hand</h4>
                            {coinList}
                        </div>
                    </div>
                }
                <button onClick={this.props.onSummaryPhaseComplete}>Continue</button>
                {this.state.showGroupedTricks ? this.renderModal() : <div></div> }
            </div>
        );
    }
}