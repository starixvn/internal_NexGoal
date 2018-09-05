import React from 'react';

import THead from './THead';
import TBody from './TBody';
import Row from './Row';

const Table = (props) => {
    return (
        <table className="table table-bordered assign-permission">
            <THead />
            <TBody>
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
            </TBody>
        </table>
    );
};

export default Table;