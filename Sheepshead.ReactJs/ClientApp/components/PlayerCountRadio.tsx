import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface PlayerCountState {
    title: string;
    value: number;
    onChange: any;
    name: string;
    remaining: number;
}

//export default class PlayerCountRadio extends React.Component<RouteComponentProps<{}>, PlayerCountState> {
export default class PlayerCountRadio extends React.Component<any, any> {
    constructor(props: PlayerCountState) {
        super(props);
        this.state = {
            value: props.value || 0,
            title: props.title,
            onChange: props.onChange,
            name: props.name,
            remaining: props.remaining
        };
        this.handleClick = this.handleClick.bind(this);
    }

    handleClick(i:number) {
        this.setState({ value: i });
        if (this.state.onChange)
            this.state.onChange(this.state.name, i);
    }

    getValue() {
        return this.state.value;
    }

    renderRadio(i: number) {
        return (
            <span>
                <input
                    type="radio"
                    name={this.state.name}
                    value={i}
                    onClick={() => this.handleClick(i)}
                    onChange={() => function () { return; }}
                    checked={this.state.value === i}
                    disabled={this.state.remaining + this.state.value < i}
                />{i}
            </span>
        );
    }

    public render() {
        return (
            <div className="playerCountRadio" onChange={this.state.onChange}>
                <span className="title">{this.state.title}</span>
                {this.renderRadio(0)}
                {this.renderRadio(1)}
                {this.renderRadio(2)}
                {this.renderRadio(3)}
                {this.renderRadio(4)}
                {this.renderRadio(5)}
                <input type="hidden" className="value" value={this.state.value} />
            </div>
        );
    }
}