import React from 'react';

import getResource from '../../../resourceHelper';
import Row from './Row';

const Table = (props) => {
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
                    props.functions.map(func => {
                        return (
                            <Row key={'function-' + func.functionId}
                                function={func}
                                isHost={props.isHost}
                                onCheckboxChange={props.onCheckboxChange}
                            />
                        );
                    })
                }
            </tbody>
        </table>
    );
};

export default Table;