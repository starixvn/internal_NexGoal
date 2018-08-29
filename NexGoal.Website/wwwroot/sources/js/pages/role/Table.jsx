import React, { Component } from 'react';

import Row from './Row';

const Header = () => (
    <thead>
        <tr>
            <th rowSpan="2">Function</th>
            <th colSpan="2">Role Permission</th>
        </tr>
        <tr>
            <th>Read</th>
            <th>Host</th>
        </tr>
    </thead>
);

const TBody = (props) => (
    <tbody>{props.children}</tbody>
);

const Table = (props) => (
    <table className="table table-bordered assign-permission">
        <Header />
        <TBody>
            {
                props.functions.map(func => {
                    return (
                        <Row key={'function-' + func.functionId}
                            function={func}
                            isHost={props.isHost}
                            onCheckboxChange={props.onCheckboxChange} />);
                })
            }
        </TBody>
    </table>
);

export default Table;