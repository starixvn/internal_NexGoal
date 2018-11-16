import React, { Component, Fragment } from 'react';

import getResource from '../../resourceHelper';

export default class Table extends Component {
    render() {
        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th className="width-30">{getResource('SymbolCount')}</th>
                        <th>{getResource('Nominee')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Status')}</th>
                        <th>{getResource('Nominator')}</th>
                        <th>{getResource('Action')}</th>
                    </tr>
                </thead>
                <tbody>

                </tbody>
            </table>
        );
    }
}