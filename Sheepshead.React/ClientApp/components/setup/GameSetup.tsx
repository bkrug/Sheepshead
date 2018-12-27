import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import PlayerCountRadio from './PlayerCountRadio';
import OnOffRadio from './OnOffRadio';
import { FetchUtils } from '../FetchUtils';

export interface GameSetupState {
    playerCount: number;
    partnerCard: boolean;
    remaining: number;
    gameName: string;
    selections: { [index: string]: number };
    leastersDefault: boolean;
    version: string;
}

export class GameSetup extends React.Component<RouteComponentProps<{}>, GameSetupState> {
    readonly MAX_PLAYERS = 5;
    readonly HUMANS = "humanCount";
    readonly SIMPLE = "simpleCount";
    readonly INTERMEDIATE = "intermediateCount";
    readonly ADVANCED = "advancedCount";

    constructor(props: any) {
        super(props);
        let selections: { [index: string]: number } = {};
        selections[this.HUMANS] = 1;
        selections[this.SIMPLE] = selections[this.INTERMEDIATE] = selections[this.ADVANCED] = 0;
        this.state = {
            playerCount: 1,
            partnerCard: true,
            remaining: this.MAX_PLAYERS,
            gameName: '',
            selections: selections,
            leastersDefault: this.getLeastersDefault(props),
            version: '0.x.0.0'
        };
        this.handleChange = this.handleChange.bind(this);
        var self = this;
        FetchUtils.get(
            'Setup/GetVersion',
            function (receivedVersion: string): void {
                self.setState({
                    //version: receivedVersion.major + '.' + receivedVersion.minor + '.' + receivedVersion.revision + '.' + receivedVersion.build
                    version: receivedVersion
                });
            });
    }

    private getLeastersDefault(props: any): boolean {
        var queryParams = props.location.search
            .replace('?', '')
            .split('&')
            .map(function (q: string) { return q.split('='); });
        var leastersParam = queryParams.filter(function (kvp: string) { return kvp[0] == 'leastersOn' });
        return leastersParam.length == 0 ? true : leastersParam[0][1].toLowerCase() == 'true';
    }

    handleChange(radioGroup: string, radioValue: number) {
        if (radioValue === 0 || radioValue > 0) {
            this.state.selections[radioGroup] = radioValue;
            let newTotal = this.state.selections[this.HUMANS]
                + this.state.selections[this.SIMPLE]
                + this.state.selections[this.INTERMEDIATE]
                + this.state.selections[this.ADVANCED];
            this.setState({ playerCount: newTotal, remaining: this.MAX_PLAYERS - newTotal });
        }
    }

    playerCountValidityStyle() {
        return this.validPlayerCount() ? '' : 'invalid';
    }

    validPlayerCount() {
        return (this.state.playerCount === 3 || this.state.playerCount === 5);
    }

    public render() {
        return (
            <div className="game-setup page-contents">
                <div className="centered-page-contents">
                    <h4>Setup Sheepshead Game</h4>
                    <form method="post">
                        <PlayerCountRadio name={this.HUMANS} title="Humans" onChange={this.handleChange} value={this.state.selections[this.HUMANS]} remaining={this.state.remaining} />
                        <PlayerCountRadio name={this.SIMPLE} title="A.I. Simple" onChange={this.handleChange} value={this.state.selections[this.SIMPLE]} remaining={this.state.remaining} />
                        <PlayerCountRadio name={this.INTERMEDIATE} title="A.I. Intermediate" onChange={this.handleChange} value={this.state.selections[this.INTERMEDIATE]} remaining={this.state.remaining} />
                        <PlayerCountRadio name={this.ADVANCED} title="A.I. Advanced" onChange={this.handleChange} value={this.state.selections[this.ADVANCED]} remaining={this.state.remaining} />
                        <div className="total-players">
                            <span>Total Players:</span>
                            <span className={"totalPlayers " + this.playerCountValidityStyle()}>{this.state.playerCount}</span>
                        </div>
                        <input type="hidden" className="remaining" value={this.state.remaining} />
                        <OnOffRadio name="partnerCard" title="Partner Card" onText="Jack of Hearts" offText="Called Ace" defaultValue={true} disabled={this.state.playerCount != 5} />
                        <OnOffRadio name="leastersGame" title="Leasters On" onText="on" offText="off" defaultValue={this.state.leastersDefault} disabled={false} />
                        <input type="submit" value="Play" disabled={!this.validPlayerCount()} />
                    </form>
                    <div className="directions-parent">
                        <a href="/directions">Directions</a>
                    </div>
                </div>
                <div className="source-code">
                    <div>See the source code at <a href="https://github.com/bkrug/Sheepshead">Github</a></div>
                    <div>{this.state.version}</div>
                </div>
            </div>
        );
    }
}