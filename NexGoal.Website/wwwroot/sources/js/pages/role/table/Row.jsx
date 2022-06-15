import React from 'react';
import { Policy } from '../../../constants';

const Row = props => {
    const renderCellFunctionName = func => {
        if (func.parentId) {
            return (
                <td>
                    <span className="icon-treedot" />
                    <span className="func-name">{func.name}</span>
                </td>
            );
        }

        return (
            <td className="level-first">
                <span className="icon-menu-down" />
                <span className="func-name">{func.name}</span>
            </td>
        );
    };

    const onChange = event => {
        props.onChange(props.function, event.target.name, event.target.checked);
    };

    const functionObj = props.function;

    return (
        <tr className={!functionObj.parentId ? 'row-special' : ''}>
            {renderCellFunctionName(functionObj)}

            <td className="text-center">
                {
                    (functionObj.onlyPolicy === null || functionObj.onlyPolicy === Policy.Read) &&
                    <div className="checkbox">
                        <input type="checkbox" name="funcRead" className="checkbox-custom"
                            disabled={!props.isHost}
                            checked={functionObj.policy > Policy.None}
                            onChange={onChange} id={`chb-read-${functionObj.functionId}`}
                        />
                        <label htmlFor={`chb-read-${functionObj.functionId}`} />
                    </div>
                }
            </td>
            <td className="text-center">
                {
                    (functionObj.onlyPolicy === null || functionObj.onlyPolicy === Policy.Host) &&
                    <div className="checkbox">
                        <input type="checkbox" name="funcHost" className="checkbox-custom"
                            disabled={!props.isHost}
                            checked={functionObj.policy === Policy.Host}
                            onChange={onChange} id={`chb-host-${functionObj.functionId}`}
                        />
                        <label htmlFor={`chb-host-${functionObj.functionId}`} />
                    </div>
                }
            </td>
        </tr>
    );
};

export default Row;