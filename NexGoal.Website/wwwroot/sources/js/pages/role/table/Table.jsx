import React, { Component } from 'react';

import getResource from '../../../resourceHelper';
import Row from './Row';

export default class Table extends Component {
    render() {
        return (
            <table className="table table-bordered assign-permission">
                <thead>
                    <tr>
                        <th rowSpan="2">{getResource('Function')}</th>
                        <th colSpan="2">{getResource('RolePermission')}</th>
                    </tr>
                    <tr>
                        <th>{getResource('Read')}</th>
                        <th>{getResource('Host')}</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.props.functions.map(func => {
                            return (
                                <Row key={'function-' + func.functionId}
                                    function={func}
                                    isHost={this.props.isHost}
                                    onCheckboxChange={this.props.onCheckboxChange}
                                />
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}