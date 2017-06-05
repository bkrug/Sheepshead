import React from 'react';

export default class Suffle extends React.Component {
    render() {
        return (
            <div>
                <Stack title="blind" count="1" />
                <Stack title="Frank" count="2" />
                <Stack title="Mellisa" count="0" />
                <Stack title="Joel" count="3" />
                <Stack title="Roberto" count="6" />
                <Stack title="Vivek" count="5" />
            </div>
        );
    }
}

class Stack extends React.Component {
    renderFullCardBack() {
        return this.props.count > 0 ? (<div className="card-back-full" />) : (<div className="card-empty" />);
    }

    renderPartialCardBack(i) {
        return this.props.count >= i ? (<div className="card-back-part" />) : (<div />);
    }

    render() {
        return (
            <div className="card-stack">
                <h3>{this.props.title}</h3>
                {this.renderFullCardBack()}
                {this.renderPartialCardBack(2)}
                {this.renderPartialCardBack(3)}
                {this.renderPartialCardBack(4)}
                {this.renderPartialCardBack(5)}
                {this.renderPartialCardBack(6)}
            </div>
        )
    }
}