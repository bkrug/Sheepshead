import '../../css/game.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface CheatState {
}

export interface CheatProps extends React.Props<any> {
}

export class CheatSheet extends React.Component<CheatProps, CheatState> {
    render() {
        return (
            <table>
                <tbody>
                    <tr className='borderless'>
                        <td><b>Power</b></td>
                        <td colSpan={8} style={{ textAlign: 'left', verticalAlign: 'bottom' }}>←More</td>
                        <td colSpan={6} style={{ textAlign: 'right', verticalAlign: 'bottom' }}>Less→</td>
                    </tr>
                    <tr>
                        <td><h5>Trump</h5></td>
                        <td className='blkCard'>Q♣</td>
                        <td className='blkCard'>Q♠</td>
                        <td className='redCard'>Q♥</td>
                        <td className='redCard'>Q♦</td>
                        <td className='blkCard'>J♣</td>
                        <td className='blkCard'>J♠</td>
                        <td className='redCard'>J♥</td>
                        <td className='redCard'>J♦</td>
                        <td className='redCard'>A♦</td>
                        <td className='redCard'>10♦</td>
                        <td className='redCard'>K♦</td>
                        <td className='redCard'>9♦</td>
                        <td className='redCard'>8♦</td>
                        <td className='redCard'>7♦</td>
                    </tr>
                    <tr>
                        <td><h5>Club</h5></td>
                        <td colSpan={8}></td>
                        <td className='blkCard'>A♣</td>
                        <td className='blkCard'>10♣</td>
                        <td className='blkCard'>K♣</td>
                        <td className='blkCard'>9♣</td>
                        <td className='blkCard'>8♣</td>
                        <td className='blkCard'>7♣</td>
                    </tr>
                    <tr>
                        <td><h5>Spades</h5></td>
                        <td colSpan={8}></td>
                        <td className='blkCard'>A♠</td>
                        <td className='blkCard'>10♠</td>
                        <td className='blkCard'>K♠</td>
                        <td className='blkCard'>9♠</td>
                        <td className='blkCard'>8♠</td>
                        <td className='blkCard'>7♠</td>
                    </tr>
                    <tr>
                        <td><h5>Hearts</h5></td>
                        <td colSpan={8}></td>
                        <td className='redCard'>A♥</td>
                        <td className='redCard'>10♥</td>
                        <td className='redCard'>K♥</td>
                        <td className='redCard'>9♥</td>
                        <td className='redCard'>8♥</td>
                        <td className='redCard'>7♥</td>
                    </tr>
                    <tr>
                        <td><b>Points</b></td>
                        <td colSpan={4}>3</td>
                        <td colSpan={4}>2</td>
                        <td>11</td>
                        <td>10</td>
                        <td>4</td>
                        <td colSpan={3}>0</td>
                    </tr>
                </tbody>
            </table>
        );
    }
}